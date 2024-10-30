using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands.Group;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge.Handlers.Group;

public class UpdateGroupHandler : IRequestHandler<UpdateGroupCommand, GroupEntity>
{
    private readonly IGroupRepository _groupRepository;
    public UpdateGroupHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public Task<GroupEntity> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        
        
        throw new System.NotImplementedException();
    }
}