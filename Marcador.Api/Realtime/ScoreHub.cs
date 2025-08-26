using Microsoft.AspNetCore.SignalR;

namespace Marcador.Api.Realtime;

public class ScoreHub : Hub
{
    private const string Prefix = "match:";

    public Task JoinMatch(string matchId)
        => Groups.AddToGroupAsync(Context.ConnectionId, Prefix + matchId);

    public Task LeaveMatch(string matchId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, Prefix + matchId);

    public Task BroadcastState(string matchId, object state)
        => Clients.Group(Prefix + matchId).SendAsync("state", state);
}
