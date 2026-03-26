import api from './axios'

export interface CreateUserPayload {
  fullName: string
  email: string
  password: string
  role: string
}

export const getUsersRequest = async () => {
  const { data } = await api.get('/users')
  return data
}

export const createUserRequest = async (payload: CreateUserPayload) => {
  const { data } = await api.post('/users', payload)
  return data
}

export const deleteUserRequest = async (id: string) => {
  const { data } = await api.delete(`/users/${id}`)
  return data
}