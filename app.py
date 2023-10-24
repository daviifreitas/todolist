import datetime
import jwt
from flask import Flask, jsonify, request, make_response
from flask_swagger_ui import get_swaggerui_blueprint
from functools import wraps
from jwt import encode, decode
import unittest


from Entities.Task import Task

todo_tasks_list = []

import json

app = Flask(__name__)

app.config['KEY'] = '7e744e429a0f4c43bd1468ad70637905'
default_user_username = 'tester01'
default_user_password = 'password1'


def authorize(func):
    @wraps(func)
    def wrapped(*args, **kwargs):
        token = request.headers.get('token')
        if not token:
            return jsonify({'message': 'Missing token'}), 403
        try:
            data = jwt.decode(token, app.config['KEY'])
        except:
            return jsonify({'message': 'Invalid token'}), 403
        return func(*args, **kwargs)

    return wrapped


SWAGGER_URL = '/swagger'
API_URL = '/static/swagger.json'

swagger_ui_blueprint = get_swaggerui_blueprint(
    SWAGGER_URL,
    API_URL,
    config={
        'app_name': 'todo_list'
    }
)
app.register_blueprint(swagger_ui_blueprint, url_prefix=SWAGGER_URL)


@app.route('/public')
def public():
    return 'Anyone can see this'

@app.route('/task', methods=['GET'])
@authorize
def get_all():
    if todo_tasks_list is None:
        return jsonify({'message': 'Tasks not found!'}, 404)
    return jsonify(todo_tasks_list, 200)


@app.route('/task/<int:id>', methods=['GET'])
@authorize
def get_task(id):
    for obj in todo_tasks_list:
        if obj.id == id:
            return jsonify(obj.to_json(), 200)
    return jsonify({'message': 'Task not found!'}, 404)

@app.route('/task/edit/<int:id>', methods=['GET'])
@authorize
def update_task(id):
    try:
        receive_update_data = request.form
        if receive_update_data is None:
            return jsonify({'message': 'Empty data for create a new task!'}), 204
        new_task_title, new_task_description = receive_update_data.get('title'), receive_update_data.get('description')
        new_task_assigned = receive_update_data.get('assigned')
        for obj in todo_tasks_list:
            if obj.id == id:
                obj.title = new_task_title
                obj.description = new_task_description
                obj.assigned = new_task_assigned
                return jsonify({'message': 'Task updated with sucessful!'}), 200
        return jsonify({'message': 'Task not found!'}, 404)
    except:
        return jsonify({'message': 'Something is wrong for update this task!'}), 500

@app.route('/task/create', methods=['POST'])
@authorize
def create_task():
    try:
        receive_data = request.form
        if receive_data is None:
            return jsonify({'message': 'Empty data for create a new task!'}), 204
        new_task_title, new_task_description = receive_data.get('title'), receive_data.get('description')
        new_task_assigned = receive_data.get('assigned')
        new_task = Task(new_task_title, new_task_description, new_task_assigned)
        todo_tasks_list.append(new_task)
        return jsonify({'message': 'Created a new task with successful!'}), 201
    except:
        return jsonify({'message': 'Something is wrong for create a new task!'}), 500


@app.route('/auth', methods=['POST'])
@authorize
def authorize():
    data = request.form
    username_for_login, password_for_login = data.get('name'), data.get('password')
    if (username_for_login == default_user_username and password_for_login == default_user_password):
        token = jwt.encode({
            'user_public_id': 1,
            'exp': datetime.datetime.now() + datetime.timedelta(minutes=30)
        }, app.config('KEY'))
        return make_response(jsonify({'token': token.decode('UTF-8')}), 201)


if __name__ == '__main__':
    app.run()
