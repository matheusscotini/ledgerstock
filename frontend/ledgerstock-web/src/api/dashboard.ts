import api from './axios'

export const getDashboardSummaryRequest = async () => {
  const { data } = await api.get('/dashboard/summary')
  return data
}

export const getLowStockProductsRequest = async () => {
  const { data } = await api.get('/dashboard/low-stock-products')
  return data
}