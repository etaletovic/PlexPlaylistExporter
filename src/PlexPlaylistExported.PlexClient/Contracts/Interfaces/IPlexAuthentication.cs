﻿namespace PlexPlaylistExporter.PlexClient.Contracts.Interfaces
{
    public interface IPlexAuthentication
    {
        Task<IDictionary<string, string>> GetHeaders();
    }
}