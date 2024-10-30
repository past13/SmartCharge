using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands;
using SmartCharge.Domain.Entities;
using SmartCharge.Repository;

namespace SmartCharge.Handlers;

public class UpdateGroupHandler : IRequestHandler<CreateGroupCommand, GroupEntity>
{
    private readonly IGroupRepository _groupRepository;
    public UpdateGroupHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public Task<GroupEntity> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        
        
        throw new System.NotImplementedException();
    }
}