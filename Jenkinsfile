pipeline {
    agent any
    stages {     
       stage('Preparation') {
        sh 'cd /home/Docker/netcorexamples; docker-compose down --rmi all'
    }
    stage('Run') {
       sh 'cd /home/Docker/netcorexamples; docker-compose up -d'
    }
}