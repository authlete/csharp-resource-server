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


using Authlete.Dto;


namespace ResourceServer.Db
{
    /// <summary>
    /// Dummy database operations for a user table.
    /// </summary>
    public class UserDao
    {
        // Dummy user database table.
        static readonly UserEntity[] USER_DB =
        {
            new UserEntity
            {
                Subject     = "1001",
                Name        = "John Smith",
                Email       = "john@example.com",
                Address     = new Address { Country = "USA"},
                PhoneNumber = "+1 (425) 555-1212"
            },
            new UserEntity
            {
                Subject     = "1002",
                Name        = "Jane Smith",
                Email       = "jane@example.com",
                Address     = new Address { Country = "Chile"},
                PhoneNumber = "+56 (2) 687 2400"
            }
        };


        /// <summary>
        /// Get a user entity by a subject.
        /// </summary>
        public static UserEntity GetBySubject(string subject)
        {
            // For each record in the dummy user database table.
            foreach (UserEntity entity in USER_DB)
            {
                // If the subjects match.
                if (entity.Subject.Equals(subject))
                {
                    // Found the user who has the subject.
                    return entity;
                }
            }

            // Not found any user who has the subject.
            return null;
        }
    }
}
