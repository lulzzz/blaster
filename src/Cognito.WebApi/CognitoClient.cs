using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;

namespace Cognito.WebApi
{
    public class CognitoClient
    {
        private readonly AmazonCognitoIdentityProviderClient _identityProviderClient;
        private readonly string _userPoolId;


        public CognitoClient(string accessKey, string secretKey, string poolName)
        {
            var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            _identityProviderClient =
                new AmazonCognitoIdentityProviderClient(awsCredentials, RegionEndpoint.EUCentral1);


            _userPoolId = CreateUserPoolAsync(poolName).Result;
        }


        public async Task AddUserToGroup(
            string username,
            string groupName
        )
        {
            var userToGroupRequest = new AdminAddUserToGroupRequest
            {
                Username = username,
                GroupName = groupName,
                UserPoolId = _userPoolId
            };


            await _identityProviderClient.AdminAddUserToGroupAsync(userToGroupRequest);
        }


        public async Task CreateUser(string userName)
        {
            var createUserRequest = new AdminCreateUserRequest
            {
                UserPoolId = _userPoolId,
                Username = userName,
                MessageAction = MessageActionType.SUPPRESS,
                //    UserAttributes = new List<AttributeType> {new AttributeType {Name = "shoe-color", Value = "brown"}}
            };

            await _identityProviderClient.AdminCreateUserAsync(createUserRequest);
        }

        public async Task CreateGroupAsync(string groupName)
        {
            var createGroupRequest = new CreateGroupRequest();
            createGroupRequest.UserPoolId = _userPoolId;
            createGroupRequest.GroupName = groupName;
            await _identityProviderClient.CreateGroupAsync(createGroupRequest);
        }


        public async Task DeleteUserPoolAsync()
        {
            var deleteUserPoolRequest = new DeleteUserPoolRequest();
            deleteUserPoolRequest.UserPoolId = _userPoolId;
            await _identityProviderClient.DeleteUserPoolAsync(deleteUserPoolRequest);
        }


        public async Task<string> CreateUserPoolAsync(string poolName)
        {
            var createUserPoolRequest = new CreateUserPoolRequest
            {
                PoolName = poolName
            };

            var response = await _identityProviderClient.CreateUserPoolAsync(createUserPoolRequest);


            return response.UserPool.Id;
        }

        public async Task<GroupType> GetGroupAsync(string groupName)
        {
            var getGroupRequest = new GetGroupRequest
            {
                GroupName = groupName,
                UserPoolId = _userPoolId
            };

            try
            {
                var getGroupResponse = await _identityProviderClient.GetGroupAsync(getGroupRequest);


                return getGroupResponse.Group;
            }
            catch (ResourceNotFoundException)
            {
                return null;
            }
        }
        
        
        public async Task<IEnumerable<string>> ListGroupsAsync()
        {
            var groupNames = new List<string>();
            string nextToken = null;
            do
            {
                var listGroupsRequest = new ListGroupsRequest
                {
                    UserPoolId = _userPoolId,
                    NextToken = nextToken
                };

                var listGroupsResponse = await _identityProviderClient.ListGroupsAsync(listGroupsRequest);
                nextToken = listGroupsResponse.NextToken;

                groupNames.AddRange(listGroupsResponse.Groups.Select(g => g.GroupName));
            } while (nextToken != null);


            return groupNames;
        }
    }
}