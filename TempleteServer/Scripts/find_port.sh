#!/bin/bash

PORT_START=20000
PORT_END=20100

for port in $(seq $PORT_START $PORT_END); do
    if ! ss -tlnp | grep -q ":${port} "; then
        echo $port
        exit 0
    fi
done

echo "ERROR: No available port" >&2
exit 1