require 'rake'
require 'configatron'

configatron.role = 'bundler'
configatron.solution = Proc.new { configatron.role + '.sln' }

task default: :compile

desc "Compiles #{configatron.role!}"
task :compile do
  msbuild = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', 'v4.0.30319', 'msbuild.exe')

  cmd = [msbuild, configatron.solution!.call, '/target:Rebuild', '/property:Configuration=Debug']

  sh(*cmd)
end

desc "Runs specs for #{configatron.role!}"
task spec: :compile do
  puts 'spec'
end
