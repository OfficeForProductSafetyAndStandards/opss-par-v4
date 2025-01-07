using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Queries.TestQueries;

public class GetTestDataQuery : IRequest<ActionResult<TestDataDto>>
{ }
