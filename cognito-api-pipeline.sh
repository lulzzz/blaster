#!/bin/bash
#
# build.sh(1)
#

[[ -n $DEBUG ]] && set -x
set -eu -o pipefail

# build parameters
readonly REGION=${AWS_DEFAULT_REGION:-"eu-central-1"}
readonly IMAGE_NAME='cognito-api'
readonly BUILD_NUMBER=${1:-"N/A"}
readonly BUILD_SOURCES_DIRECTORY=${2:-${PWD}}

restore_dependencies() {
    echo "Restoring dependencies"
    dotnet restore Cognito.sln
}

run_tests() {
    echo "Running tests..."
    dotnet build -c Release Cognito.sln
    dotnet test --logger:"trx;LogFileName=testresult.trx" --results-directory "../" Cognito.Tests/Cognito.Tests.csproj
}

publish_binaries() {
    echo "Publishing binaries..."
    dotnet publish -c Release -o ${BUILD_SOURCES_DIRECTORY}/output Cognito.WebApi/Cognito.WebApi.csproj
}


build_container_image() {
    echo "Building container image..."
    
    docker build -t ${IMAGE_NAME} --file Dockerfile.Cognito .
}

push_container_image() {
    echo "Login to docker..."
    $(aws ecr get-login --no-include-email)

    account_id=$(aws sts get-caller-identity --output text --query 'Account')
    image_name="${account_id}.dkr.ecr.${REGION}.amazonaws.com/ded/${IMAGE_NAME}:${BUILD_NUMBER}"

    echo "Tagging container image..."
    docker tag ${IMAGE_NAME}:latest ${image_name}

    echo "Pushing container image to ECR..."
    docker push ${image_name}
}

cd ./src

restore_dependencies
# run_tests
publish_binaries

cd ..

build_container_image

if [[ "${BUILD_NUMBER}" != "N/A" ]]; then
    push_container_image
fi