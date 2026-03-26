import api from './axios'

export interface ProductPayload {
  name: string
  sku: string
  description?: string
  category?: string
  price: number
  minimumStock: number
}

export interface UpdateProductPayload extends ProductPayload {
  isActive: boolean
}

export const getProductsRequest = async (params?: {
  search?: string
  isActive?: boolean | ''
}) => {
  const { data } = await api.get('/products', { params })
  return data
}

export const getProductByIdRequest = async (id: string) => {
  const { data } = await api.get(`/products/${id}`)
  return data
}

export const createProductRequest = async (payload: ProductPayload) => {
  const { data } = await api.post('/products', payload)
  return data
}

export const updateProductRequest = async (id: string, payload: UpdateProductPayload) => {
  const { data } = await api.put(`/products/${id}`, payload)
  return data
}

export const toggleProductStatusRequest = async (id: string) => {
  const { data } = await api.patch(`/products/${id}/toggle-status`)
  return data
}