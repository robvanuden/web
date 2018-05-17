// Copyright Matthias Koch 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/web/blob/master/LICENSE

using System;
using System.Linq;

class ApiProject
{
    public string PackageId { get; set; }
    public bool IsExternalRepository => RepositoryUrl != "https://github.com/nuke-build/nuke";
    public string RepositoryUrl { get; set; }
}
