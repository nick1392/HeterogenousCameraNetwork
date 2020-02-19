# HeterogenousCameraNetwork
Heterogenous camera network, with fixed PTZ and UAV-mounted cameras

The simulator structure can be divided in two main layers:
1. Crowd behavior simulation
2. Camera network deploying and control

## Crowd behavior simulation
The movement of pedestrian in the environments is based on the [Social Force model](https://arxiv.org/pdf/cond-mat/9805244).
The visula appearance of the crowd is generated using the [UMA library](https://github.com/umasteeringgroup/UMA)

## Camera network deploying and control

## Requirements

1. [Unity 2019.1.0f2](https://unity3d.com/unity/whats-new/2019.1.0)
2. Python 3.6 or greater
3. Mlagents v0.12, `pip install mlagents==0.12.1`
