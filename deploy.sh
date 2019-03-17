#!/usr/bin/env bash

dir=$(pwd)
cdir=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

# deploy variables
remote_user=mattjcowan
remote_server=remote.mattjcowan
remote_service=site.mattjcowan
remote_path=/var/www/mattjcowan.com/html

# publish local dist directory to remote server
local_path=$cdir/../dist
ssh $remote_user@$remote_server "sudo chown -R $remote_user:$remote_user $remote_path"
rsync -avz --delete --no-perms -e 'ssh' $local_path/ $remote_user@$remote_server:$remote_path
ssh $remote_user@$remote_server "sudo chown -R www-data:www-data $remote_path/ && sudo chmod -R 755 $remote_path/"
ssh $remote_user@$remote_server "sudo service $remote_service restart"
