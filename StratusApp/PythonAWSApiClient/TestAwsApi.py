import boto3
import json
import os
import sys
import time
import traceback
from datetime import datetime
from pathlib import Path
from typing import Any, Callable, Dict, List, Optional, Tuple, Union


# Set up the AWS credentials and region
def printInstanceData():
    session = boto3.Session(
        aws_access_key_id='',
        aws_secret_access_key='',
        region_name='us-east-1'
    )

    # Create an EC2 client
    ec2 = session.client('ec2')
    cloudwatch = session.client('cloudwatch')
    # Get the EC2 instance usage data
    response = cloudwatch.get_metric_statistics(
        Namespace='AWS/EC2',
        MetricName='CPUUtilization',
        Dimensions=[
            {
                'Name': 'InstanceId',
                'Value': 'Enter the Instance Id here'
            },
        ],
        StartTime='2023-02-26T00:00:00Z',
        EndTime='2023-02-28T23:59:59Z',
        Period=86400,
        Statistics=['Minimum', 'Maximum', 'Average', 'Sum'],
    )
    #network_in = response["Reservations"][0]["Instances"][0]["NetworkInterfaces"][0]["Attachment"]["NetworkCardIndex"]
    #network_out = response["Reservations"][0]["Instances"][0]["NetworkInterfaces"][0]["Attachment"]["NetworkCardIndex"]
    # Print the instance data
    datapoints = response['Datapoints']

    if datapoints:
        avg_cpu_usage = datapoints[0]['Average']
        max_cpu_usage = datapoints[0]['Maximum']
        min_cpu_usage = datapoints[0]['Minimum']
        sum_cpu_usage = datapoints[0]['Sum']
    else:
        avg_cpu_usage = 0.0

    print(f"Avg cpu usage: {avg_cpu_usage}, Max cpu usage: {max_cpu_usage}, Min cpu usage: {min_cpu_usage}, Sum cpu usage: {sum_cpu_usage}")


def showPricesOfVms():
    import boto3

    # Get current instance data
    ec2_client = boto3.client('ec2')

    response = ec2_client.describe_instances()
    current_instance_type = response['Reservations'][0]['Instances'][0]['InstanceType']
    current_instance_id = response['Reservations'][0]['Instances'][0]['InstanceId']

    print(f"Current Instance: {current_instance_id} - Type: {current_instance_type}")

    # Get possible instance types and prices
    pricing_client = boto3.client('pricing')

    response = pricing_client.get_products(
        ServiceCode='AmazonEC2',
        Filters=[
            {'Type': 'TERM_MATCH', 'Field': 'operatingSystem', 'Value': 'Linux'},
            {'Type': 'TERM_MATCH', 'Field': 'preInstalledSw', 'Value': 'NA'},
            {'Type': 'TERM_MATCH', 'Field': 'capacitystatus', 'Value': 'Used'},
            {'Type': 'TERM_MATCH', 'Field': 'tenancy', 'Value': 'Shared'},
            {'Type': 'TERM_MATCH', 'Field': 'location', 'Value': 'US East (N. Virginia)'}
        ],
        MaxResults=100
    )

    instance_data = {}

    for product in response['PriceList']:
        sku = product['product']['sku']
        instance_type = product['product']['attributes']['instanceType']
        instance_family = product['product']['attributes']['instanceFamily']
        usage_type = list(product['terms']['OnDemand'].keys())[0]
        price = float(product['terms']['OnDemand'][usage_type]['priceDimensions']['USD']['pricePerUnit']['USD'])

        if instance_family not in instance_data:
            instance_data[instance_family] = {}

        instance_data[instance_family][instance_type] = {
            'SKU': sku,
            'UsageType': usage_type,
            'Price': price
        }

    # Compare current instance with possible instances
    current_instance_family = current_instance_type.split('.')[0]
    current_instance_price = instance_data[current_instance_family][current_instance_type]['Price']

    print("Possible Instances:")

    for instance_type in instance_data[current_instance_family]:
        price = instance_data[current_instance_family][instance_type]['Price']

        if price < current_instance_price:
            print(f" - {instance_type}: $ {price:.2f} (Cheaper than current instance)")
        elif price > current_instance_price:
            print(f" - {instance_type}: $ {price:.2f} (More expensive than current instance)")
        else:
            print(f" - {instance_type}: $ {price:.2f} (Same price as current instance)")


if __name__ == '__main__':
    #printInstanceData()
    showPricesOfVms()
