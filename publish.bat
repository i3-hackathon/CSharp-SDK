@set NUGET=Build\NuGet

@echo =========================
@echo NuGet package publishing.

@for %%g in (Build\BMW.Client.*.nupkg) do @(
  @echo Publishing %%g
  @%NUGET% Push %%g
)

@echo BMW.Client publishing completed.
@goto end

:end
@pause