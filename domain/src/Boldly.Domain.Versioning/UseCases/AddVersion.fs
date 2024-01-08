module Boldly.Domain.Versioning.UseCases.AddVersion

open System
open Boldly.Domain.Common
open Boldly.Domain.Common.Name
open Boldly.Domain.Common.Types
open Boldly.Domain.Common.Types.ResultExtensions
open Boldly.Domain.Versioning.Types
open Boldly.Domain.Versioning.PlatformAdministration.Types
open Boldly.Domain.Versioning.PlatformAdministration.DataAccess
open DataAccess.Version
open FsToolkit.ErrorHandling
open Semver

module Types =
    type EnvironmentModel =
        | Id of Guid
        | Name of string

    type AddVersionRequestModel =
        { ApplicationId: Guid
          Value: string
          Name: string option
          ReleaseToEnvironments: EnvironmentModel list
          UserId: Guid }

    type EnvironmentIdOrName =
        | Name of Name
        | Id of EnvironmentId

    type EnvironmentNotFound = EnvironmentNotFound of EnvironmentIdOrName

    type AddVersionError =
        | ApplicationDoesNotExist of ApplicationId
        | InvalidEnvironmentNames of string list
        | EnvironmentsDontExist of EnvironmentIdOrName list
        | VersionAlreadyExists of VersionId
        | InvalidVersionName of string

    type AddVersion = AddVersionRequestModel -> AsyncResult<VersionId, AddVersionError>
    
    type FindEnvironmentError =
    | InvalidEnvironmentName of string
    | EnvironmentsDoesNotExist of EnvironmentIdOrName

open Types

let private toResult error option =
    match option with
    | Some v -> Result.Ok v
    | None -> Result.Error error
    
let findEnvironmentByName n (application: Application) =
    let name = name n |> Result.mapError (fun x -> FindEnvironmentError.InvalidEnvironmentName x)
    name
    >>= (fun x -> application.Environments
                  |> List.tryFind (fun y -> WrappedString.equals y.Name x)
                  |> toResult (EnvironmentsDoesNotExist (EnvironmentIdOrName.Name x)))

let findEnvironmentById id (application: Application) =
    application.Environments
    |> List.tryFind (fun x -> x.Id = id)
    |> toResult (EnvironmentsDoesNotExist (Id id))

let getEnvironment (environmentModel: EnvironmentModel) (app: Application) =
    match environmentModel with
    | EnvironmentModel.Name n -> app |> findEnvironmentByName n
    | EnvironmentModel.Id id -> app |> findEnvironmentById (EnvironmentId id)
    
let map (r: Result<Environment, FindEnvironmentError> list) : Result<Environment list, AddVersionError> =
    let errors =
        r
        |> List.choose (function
            | Error e -> Some e
            | Ok _ -> None)
        
    let invalidEnvironmentNames =
        errors
        |> List.choose (function | InvalidEnvironmentName name -> Some name | _ -> None)
        
    let notExistingEnvironments = errors |> List.choose (function | EnvironmentsDoesNotExist idOrName -> Some idOrName | _ -> None)
        
    let environments =
        r
        |> List.choose (function
            | Ok v -> Some v
            | Error _ -> None)

    match invalidEnvironmentNames, notExistingEnvironments with
    | [], [] -> Result.Ok environments
    | n, [] -> Result.Error (InvalidEnvironmentNames n)
    | _, n -> Result.Error (EnvironmentsDontExist n)

let parseEnvironments app releaseToEnvironments  =
    releaseToEnvironments
    |> List.map (fun x -> getEnvironment x app)
    |> map

let addVersionInternal
    (application: Result<Application, LoadApplicationError>)
    (version: Option<Version>)
    (requestModel: AddVersionRequestModel)
    = result {
        let! app = application |> Result.mapError (function | NotFound appId -> ApplicationDoesNotExist appId)
       
        let! environments = parseEnvironments app requestModel.ReleaseToEnvironments

        let! _ = match version with
                        | Some v -> Result.Error (VersionAlreadyExists v.Id)
                        | None -> Result.Ok ()
                        
        let! name = match requestModel.Name with
                            | Some n -> name n |> Result.mapError AddVersionError.InvalidVersionName |> Result.map Some
                            | None -> Ok None

        let version: Version =
            { Id = VersionId(Guid.NewGuid())
              Application = app 
              ReleasedTo = environments
              Value = VersionValue.SemanticVersion(SemVersion.Parse(requestModel.Value))
              State = Candidate
              Name = name
              Changes = []
              CreatedAt = DateTime.UtcNow
              CreatedBy = UserId(requestModel.UserId) }

        return version.Id
    }    

let interactor (findApplication: LoadApplication) (findVersion: FindVersion) : AddVersion =
    fun requestModel ->
        async {

            let! application = findApplication (ApplicationId requestModel.ApplicationId)
            let! version = findVersion (requestModel.Value.Trim())

            return addVersionInternal application version requestModel
        }
