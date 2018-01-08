//
// Copyright (C) 2018 Authlete, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific
// language governing permissions and limitations under the
// License.
//


using Authlete.Handler.Spi;
using ResourceServer.Db;


namespace ResourceServer.Spi
{
    public class UserInfoRequestHandlerSpiImpl
        : UserInfoRequestHandlerSpiAdapter
    {
        bool _tried;
        UserEntity _userEntity;


        public override object GetUserClaimValue(
            string subject, string claimName, string languageTag)
        {
            // The user who has the subject.
            UserEntity entity = GetUserBySubject(subject);

            // If there is no user who has the subject.
            if (entity == null)
            {
                // The value of the claim is not available.
                return null;
            }

            // Get the value of the claim.
            return entity.GetClaimValue(claimName);
        }


        UserEntity GetUserBySubject(string subject)
        {
            if (_tried)
            {
                return _userEntity;
            }

            // Search the user database for the user.
            _userEntity = UserDao.GetBySubject(subject);

            // Avoid calling UserDao.GetBySubject(string) again
            // for the same subject.
            _tried = true;

            return _userEntity;
        }
    }
}
