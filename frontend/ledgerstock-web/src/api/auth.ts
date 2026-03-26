import api from './axios'

export interface RegisterPayload {
  fullName: string
  email: string
  password: string
  confirmPassword: string
}

export interface LoginPayload {
  email: string
  password: string
}

export const registerRequest = async (payload: RegisterPayload) => {
  const { data } = await api.post('/auth/register', payload)
  return data
}

export const loginRequest = async (payload: LoginPayload) => {
  const { data } = await api.post('/auth/login', payload)
  return data
}

export const getMeRequest = async () => {
  const { data } = await api.get('/auth/me')
  return data
}