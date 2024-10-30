using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Group;

public class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, ApiResponse<Unit>>
{
    private readonly IGroupRepository _groupRepository;
    public DeleteGroupHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<ApiResponse<Unit>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var response = new ApiResponse<Unit>();

        var group = await _groupRepository.GetGroupById(request.Id);
        if (group == null)
        {
            response.Error = $"A Group with the Id '{request.Id}' does not exists.";
            return response;
        }

        await _groupRepository.DeleteGroupById(request.Id);
        
        return response;
    }
}