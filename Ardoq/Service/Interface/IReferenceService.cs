﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardoq.Models;
using Refit;

namespace Ardoq.Service.Interface
{
    public interface IReferenceService
    {
        [Get("/api/reference")]
        Task<List<Reference>> GetAllReferences([AliasAs("org")] string org);

        [Get("/api/reference/{id}")]
        Task<Reference> GetReferenceById([AliasAs("id")] String id, [AliasAs("org")] string org);


        [Post("/api/reference")]
        Task<Reference> CreateReference([Body] Reference reference, [AliasAs("org")] string org);


        [Put("/api/reference/{id}")]
        Task<Reference> UpdateReference([AliasAs("id")] String id, [Body] Reference reference,
            [AliasAs("org")] string org);


        [Delete("/api/reference/{id}")]
        Task DeleteReference([AliasAs("id")] String id, [AliasAs("org")] string org);
    }
}