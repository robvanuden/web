// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Linq;
using Nuke.Common.Git;

class ApiProject
{
    public string RepositoryUrl { get; set; }
    public string PackageId { get; set; }

    public GitRepository Repository => GitRepository.TryParse(RepositoryUrl);
}
