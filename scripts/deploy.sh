#!/usr/bin/env bash
# `cd` to the root of your repo
cdir=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
cd $cdir
cd ..
dir=$(pwd)
cd $dir

# set some variables
local_src=$dir/src-server/app       # assumes app is at this location
local_path=$dir/dist                # local deployment location
remote_server=demoserver            # remote server ip
remote_user=root                    # remote user
remote_service=app                  # name of system.d service
remote_path=/var/www/app/dist       # deployment path

# publish app locally
cd $local_src
npm install
dotnet publish -c release -r ubuntu.16.04-x64 -o $local_path
chmod +x $local_path/app

# rsync app to remote server
ssh $remote_user@$remote_server "sudo chown -R $remote_user:$remote_user $remote_path"
rsync -avz --delete --no-perms -e 'ssh' $local_path/ $remote_user@$remote_server:$remote_path
ssh $remote_user@$remote_server "cd $remote_path && sudo npm install && sudo chown -R www-data:www-data $remote_path/ && sudo chmod -R 755 $remote_path/"
ssh $remote_user@$remote_server "sudo service $remote_service restart"