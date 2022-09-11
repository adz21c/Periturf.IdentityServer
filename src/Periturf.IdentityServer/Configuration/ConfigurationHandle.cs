//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using Periturf.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    class ConfigurationHandle : IConfigurationHandle
    {
        private readonly List<Configuration> _configurations;
        private readonly Configuration _config;

        public ConfigurationHandle(List<Configuration> configurations, Configuration config)
        {
            _configurations = configurations;
            _config = config;
        }

        public ValueTask DisposeAsync()
        {
            _configurations.Remove(_config);
            return new ValueTask();
        }
    }
}