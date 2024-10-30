using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartCharge.Commands;
using SmartCharge.Repository;

namespace SmartCharge.Handlers;

public class DeleteGroupHandler : IRequestHandler<DeleteGroupCommand>, IRequest<Unit>
{
    private readonly IGroupRepository _groupRepository;
    public DeleteGroupHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<Unit> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupById(request.Id);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID {request.Id} not found.");
        }
        
        await _groupRepository.DeleteGroupById(request.Id);
        
        return Unit.Value;
    }
}