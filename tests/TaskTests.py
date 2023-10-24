import unittest
from unittest.mock import patch

from app import Flask, app
from app import todo_tasks_list
from Entities.Task import Task
import json


def added_fake_task_in_list():
    mock_task = Task('mocktask', 'mocking fake task', 'user')
    todo_tasks_list.append(mock_task)


def verify_exist_attribute_in_json(json_data):
    if isinstance(json_data[0], dict) and 'id' in json_data[0]:
        return True
    else:
        return False


class TestGetTaskRoxute(unittest.TestCase):
    def setUp(self):
        self.app = app.test_client()
        mock_token = ('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'
                      '.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ'
                      '.KNKzPb6mABBLQDOrFB4xAW1XmsCvSAWK5OdL2r9TZkY')
        self.headers = {'token': f'{mock_token}'}

    @patch('app.jwt.decode', return_value={'user_id': 123})  # Mock jwt.decode
    def test_get_existing_task_with_valid_attributes_with_valid_token(self, mock_jwt_decode):
        added_fake_task_in_list()
        task_id = 1
        response = self.app.get(f'/task/{task_id}', headers=self.headers)
        data = json.loads(response.get_data(as_text=True))
        self.assertEqual(response.status_code, 200)
        has_id_attribute = verify_exist_attribute_in_json(data)
        self.assertTrue(has_id_attribute)

    def test_get_existing_task_with_missing_token(self):
        task_id = 1
        response = self.app.get(f'/task/{task_id}')
        data = json.loads(response.get_data(as_text=True))
        self.assertEqual(response.status_code, 403)
        self.assertEqual(data['message'], 'Missing token')

    @patch('app.jwt.decode', side_effect=Exception('Invalid token'))
    def test_get_existing_task_with_invalid_token(self, mock_jwt_decode):
        task_id = 1
        response = self.app.get(f'/task/{task_id}', headers=self.headers)
        data = json.loads(response.get_data(as_text=True))
        self.assertEqual(response.status_code, 403)
        self.assertEqual(data['message'], 'Invalid token')


if __name__ == '__main__':
    unittest.main()
