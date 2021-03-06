﻿using System.Linq;
using System.Threading.Tasks;
using Blaster.WebApi.Features.Capabilities;
using Blaster.WebApi.Features.Capabilities.Models;

namespace Blaster.Tests.TestDoubles
{
    public class StubCapabilityServiceClient : ICapabilityServiceClient
    {
        private readonly Member _member;
        private readonly Capability[] _capabilities;

        public StubCapabilityServiceClient(Member member = null, params Capability[] capabilities)
        {
            _member = member;
            _capabilities = capabilities;
        }

        public Task<CapabilitiesResponse> GetAll()
        {
            return Task.FromResult(new CapabilitiesResponse
            {
                Items = _capabilities,
            });
        }

        public Task<Capability> CreateCapability(string name, string description)
        {
            return Task.FromResult(_capabilities.First());
        }

        public Task<Capability> GetById(string id)
        {
            return Task.FromResult(_capabilities.FirstOrDefault());
        }

        public Task JoinCapability(string capabilityId, string memberEmail)
        {
            return Task.CompletedTask;
        }

        public Task LeaveCapability(string capabilityId, string memberEmail)
        {
            return Task.CompletedTask;
        }
    }
}