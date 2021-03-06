﻿using Ardoq;
using Ardoq.Models;
using Ardoq.Service.Interface;
using ArdoqTest.Helper;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArdoqTest.Service
{
    [TestFixture]
    public class ReferenceServiceTest
    {
        private IReferenceService service;
        private IArdoqClient client;

        [OneTimeSetUp]
        public void Setup()
        {
            client = TestUtils.GetClient();
            service = client.ReferenceService;
        }

        private async Task DeleteWorkspace(Workspace workspace)
        {
            await client.WorkspaceService.DeleteWorkspace(workspace.Id);
        }

        private async Task<Workspace> CrateWorkspace()
        {
            return
                await
                    client.WorkspaceService.CreateWorkspace(new Workspace("Reference Test Workspace",
                        TestUtils.GetTestProperty("modelId"), "Hello world!"));
        }

        private async Task<Component> CreateComponent(Workspace workspace, string name)
        {
            return await client.ComponentService.CreateComponent(new Component(name, workspace.Id, ""));
        }

        private Reference CreateReferenceTemplate(Workspace workspace, Component source, Component target)
        {
            return new Reference(workspace.Id, "", source.Id, target.Id, 2);
        }

        [Test]
        public async Task CreateReferenceTest()
        {
            Workspace workspace = await CrateWorkspace();
            Component source = await CreateComponent(workspace, "Source");
            Component target = await CreateComponent(workspace, "Target");
            Reference referenceTemplate = CreateReferenceTemplate(workspace, source, target);
            Reference reference = await service.CreateReference(referenceTemplate);
            Assert.NotNull(reference.Id);
            await DeleteWorkspace(workspace);
        }

        [Test]
        public async Task DeleteReferenceTest()
        {
            Workspace workspace = await CrateWorkspace();
            Component source = await CreateComponent(workspace, "Source");
            Component target = await CreateComponent(workspace, "Target");
            Reference referenceTemplate = CreateReferenceTemplate(workspace, source, target);
            Reference result = await service.CreateReference(referenceTemplate);
            await service.DeleteReference(result.Id);

            try
            {
                await service.GetReferenceById(result.Id);
                await DeleteWorkspace(workspace);
                Assert.Fail("Expected the reference to be deleted.");
            }
            catch (Refit.ApiException e)
            {
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, e.StatusCode);
            }
            await DeleteWorkspace(workspace);
        }

        [Test]
        public async Task GetReferenceTest()
        {
            Workspace workspace = await CrateWorkspace();
            Component source = await CreateComponent(workspace, "Source");
            Component target = await CreateComponent(workspace, "Target");
            Reference referenceTemplate = CreateReferenceTemplate(workspace, source, target);

            // fill the list 
            await service.CreateReference(referenceTemplate);
            List<Reference> allWorkspaces = await service.GetAllReferences();
            string id = allWorkspaces[0].Id;
            Reference reference = await service.GetReferenceById(id);
            Assert.True(id == reference.Id);
            await DeleteWorkspace(workspace);
        }

        [Test]
        public async Task UpdateReferenceTest()
        {
            Workspace workspace = await CrateWorkspace();
            Component source = await CreateComponent(workspace, "Source");
            Component target = await CreateComponent(workspace, "Target");
            Reference referenceTemplate = CreateReferenceTemplate(workspace, source, target);
            Reference reference = await service.CreateReference(referenceTemplate);
            reference.Source = reference.Target;
            reference.Target = reference.Source;
            Reference updatedReference = await service.UpdateReference(reference.Id, reference);
            Assert.True(reference.Target == updatedReference.Source);
            Assert.True(reference.VersionCounter == updatedReference.VersionCounter - 1);
            await DeleteWorkspace(workspace);
        }
    }
}