This project is used to replace the mapped shares on a computer with new targets.

The program was written to satisfy this use case:
  * users have many different names for servers (dns, FQDN, ip, shortname, etc).
  * we are trying to migrate that mess to a DFS solution
  * we didn't want to have to touch each user's computer
  
The solution was this program. It has two map files: servers and shares.

 * ServerMappings.txt - maps all the different names for a server to one name
 * ShareMappings.txt - maps old paths to new paths. It's written in terms of the server names above.

The map files are embedded in the exe for ease of deployment.

There are sample files in the project. They must have the word Sample dropped from their names to be used.
  
If the program can map a share to a new path, it will attempt to unmap the old path and remap the new path.
