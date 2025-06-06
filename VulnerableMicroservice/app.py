# VulnerableMicroservice/app.py
from flask import Flask, request, jsonify
import redis
import json

app = Flask(__name__)
queue = redis.StrictRedis(host='localhost', port=6379, db=0)

@app.route('/submit', methods=['POST'])
def submit_request():
    data = request.get_json()
    # Lack of input validation
    customer_id = data['customer_id']
    request_data = data['request_data']
    
    # SQL Injection vulnerability (simulated)
    query = f"SELECT * FROM customers WHERE customer_id = '{customer_id}'"
    print(f"Executing query: {query}")
    
    # Insecurely storing request data in Redis queue
    queue.lpush('request_queue', json.dumps(data))
    
    return jsonify({'message': 'Request submitted successfully'}), 201

@app.route('/process', methods=['GET'])
def process_request():
    # Insecurely retrieving request data from Redis queue
    request_data = queue.rpop('request_queue')
    if request_data:
        data = json.loads(request_data)
        print(f"Processing request for customer_id: {data['customer_id']}")
        return jsonify({'message': 'Request processed successfully', 'data': data}), 200
    return jsonify({'message': 'No requests to process'}), 200

if __name__ == '__main__':
    app.run(debug=True, port=5000)