require 'rake'
require 'configatron'

configatron.role = 'bundler'
configatron.solution = Proc.new { configatron.role + '.sln' }

task default: :compile

task :configure do
  def add_tools_to_path(*tools)
    tools = ['tools/*', 'tools/*/bin'] if tools.empty?

    paths = Dir[*tools].map { |path| File.expand_path(path) }
    paths << ENV['PATH']

    ENV['PATH'] = paths.join(File::PATH_SEPARATOR)
  end

  add_tools_to_path
end

task nuget_restore: :configure do
  sh('nuget', 'restore', configatron.solution!.call)
  add_tools_to_path('packages/**/tools')
end

desc "Compiles #{configatron.role!}"
task compile: :nuget_restore do
  msbuild = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', 'v4.0.30319', 'msbuild.exe')

  cmd = [msbuild, configatron.solution!.call, '/target:Rebuild', '/property:Configuration=Debug']

  sh(*cmd)
end

desc "Runs specs for #{configatron.role!}"
task spec: :compile do
  cmd = %w(nunit-console.exe /result=build/spec/spec.xml) << Dir.glob('build/spec/*Specs.dll')

  sh(*cmd.flatten)
end
