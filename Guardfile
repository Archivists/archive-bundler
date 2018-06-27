case RbConfig::CONFIG['target_os']
when /windows|bccwin|cygwin|djgpp|mingw|mswin|wince/i
  notification :gntp, :host => 'localhost'
when /linux/i
  notification :notifysend
when /mac|darwin/i
  notification :growl
end

foo

guard :foo do
  watch('*.css') { puts 'something changed' }
end

guard :bundler do
  watch('Gemfile')
end

guard :rake, task: :spec, run_on_start: true do
  watch('Rakefile')
end

guard :depend,
  output_paths: Proc.new { Dir['build/bin/*.dll'] },
  cmd: %w(bundle exec rake compile),
  run_on_start: false do
    watch(%r{^source/.*(?<!Specs)\.cs$}i)
end

guard :depend,
  output_paths:  Proc.new { Dir['build/spec/*Specs.dll', 'build/spec/spec.xml'] },
  cmd: %w(bundle exec rake spec),
  run_on_start: false do
    watch(%r{^source/.*Specs\.cs$}i)
end
