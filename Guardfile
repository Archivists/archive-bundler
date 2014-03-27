case RbConfig::CONFIG['target_os']
when /windows|bccwin|cygwin|djgpp|mingw|mswin|wince/i
  notification :gntp, :host => 'localhost'
when /linux/i
  notification :notifysend
when /mac|darwin/i
  notification :growl
end

guard :bundler do
  watch('Gemfile')
end

guard :rake, task: :default do
  watch('Rakefile')
end

guard :shell, all_on_start: false do
  watch(%r{^source/.*\.cs})      { `bundle exec rake compile` }
  watch(%r{^source/.*Specs\.cs}) { `bundle exec rake spec` }
end
