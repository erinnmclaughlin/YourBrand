
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Models;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Application.Conversations.Queries;

public record GetConversationsQuery(
    int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null)
    : IRequest<Results<ConversationDto>>
{
    public class GetConversationsQueryHandler(IConversationRepository conversationRepository) : IRequestHandler<GetConversationsQuery, Results<ConversationDto>>
    {
        public async Task<Results<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            var query = conversationRepository.GetConversations();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(
                    request.SortBy,
                    request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            query = query.Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable();

            var conversations = await query.ToListAsync(cancellationToken);

            return new Results<ConversationDto>(
                conversations.Select(message => message.ToDto()),
                totalCount);
        }
    }
}