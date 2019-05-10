# RabbitMQ-RedisAdapter

 * Receive message from external RabbitMQ. Store it in local Redis Database used key which in header.  
 * Please use k8s deploy this adapter. /k8sDeployConfig.yml is the deploy sample include configmap. 
 
## Usage samples
 * edit configmap
 * deploy with your application using k8s
 * now your application can access local redis database which have the message from RabbitMQ

## Warning
 * Note that. You don't need change source code or re-deploy for most situation.
 * Just use the container registry-intl.cn-shanghai.aliyuncs.com/slither/mqadapter:dev
 
 
 

