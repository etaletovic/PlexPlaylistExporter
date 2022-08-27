﻿namespace PlexPlaylistExporter.Core.Contracts.Services
{
    public interface IDataProtectionService
    {
        string Protect(string value);
        Task<string> ProtectAsync(string value);
        string Unprotect(string value);
        Task<string> UnprotectAsync(string value);
    }
}