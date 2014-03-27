require 'rbconfig'

source 'https://rubygems.org'

gem 'rake'
gem 'configatron'

group :development do
  gem 'guard'
  gem 'guard-bundler'
  gem 'guard-shell'
  gem 'guard-rake'

  case RbConfig::CONFIG['target_os']
  when /windows|bccwin|cygwin|djgpp|mingw|mswin|wince/i
    gem 'ruby_gntp'
    gem 'wdm'
  when /linux/i
    gem 'rb-inotify'
  when /mac|darwin/i
    gem 'rb-fsevent'
    gem 'growl'
  end
end
