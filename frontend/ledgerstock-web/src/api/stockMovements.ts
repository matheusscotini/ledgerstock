import api from './axios'

export interface CreateStockMovementPayload {
  productId: string
  type: number
  quantity: number
  reason: string
  notes?: string
}

export const getStockMovementsRequest = async (params?: {
  productId?: string
  type?: number | ''
  startDate?: string
  endDate?: string
}) => {
  const { data } = await api.get('/stockmovements', { params })
  return data
}

export const getStockMovementByIdRequest = async (id: string) => {
  const { data } = await api.get(`/stockmovements/${id}`)
  return data
}

export const createStockMovementRequest = async (payload: CreateStockMovementPayload) => {
  const { data } = await api.post('/stockmovements', payload)
  return data
}

export const exportStockMovementsCsvRequest = async (params?: {
  productId?: string
  type?: number | ''
  startDate?: string
  endDate?: string
}) => {
  const response = await api.get('/stockmovements/export-csv', {
    params,
    responseType: 'blob',
  })

  return response.data
}