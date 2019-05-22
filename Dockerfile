# Use an official Python runtime as a parent image
FROM mcr.microsoft.com/dotnet/core/runtime:2.1

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . /app

# Make port 23001 available to the world outside this container
#EXPOSE 31000:31000/tdp
#EXPOSE 31001:31001/tdp

# Run app.py when the container launches
CMD ["dotnet", "RabbitMQAdpater.dll"]