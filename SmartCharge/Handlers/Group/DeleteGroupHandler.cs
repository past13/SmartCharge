using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Domain.Response;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Group;

public class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand, Result<GroupEntity>>
{
    private readonly IGroupRepository _groupRepository;
    public DeleteGroupHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<Result<GroupEntity>> Handle(DeleteGroupCommand command, CancellationToken cancellationToken)
    {
        var response = new Result<GroupEntity>();

        var groupExist = await _groupRepository.GetGroupById(command.Id);
        if (groupExist == null)
        {
            response.Error = $"A Group with the Id {command.Id} does not exists.";
            return response;
        }
        
        response = await _groupRepository.DeleteGroupById(command.Id);
        
        return response;
    }
}