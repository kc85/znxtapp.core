
Add below code in your project .csproj file. 
This code will generate @VersionNumber and the pust build command 

 <Target Name="PostBuildMacros">
    <GetAssemblyIdentity AssemblyFiles="$(TargetPath)">
      <Output TaskParameter="Assemblies" ItemName="Targets" />
    </GetAssemblyIdentity>
    <ItemGroup>
      <VersionNumber Include="$([System.Text.RegularExpressions.Regex]::Replace(&quot;%(Targets.Version)&quot;, &quot;^(.+?)(\.0+)$&quot;, &quot;$1&quot;))" />
    </ItemGroup>
  </Target>
  <PropertyGroup>
    <PostBuildEventDependsOn>
    $(PostBuildEventDependsOn);
    PostBuildMacros;
  </PostBuildEventDependsOn>
    <PostBuildEvent>call "$(ProjectDir)build-events-scripts\post-build\upload_module.bat" @(VersionNumber) "$(SolutionDir).nuget" "$(TargetDir)" "$(ProjectDir)$(ProjectName).nuspec" $(ProjectName) "$(ProjectDir)"</PostBuildEvent>
  </PropertyGroup>


  Update your AssemblyInfo.cs below line mark * for the incremental value.

  [assembly: AssemblyVersion("1.0.1.*")]