export const selectProducts = state =>                      state.product.products
export const selectProductError = state =>                  state.product.error
export const selectProductStatus = state =>                 state.product.status

export const extractIdFromProduct = product =>              product.configId
export const extractNameFromProduct = product =>            product.name
export const extractDescriptionFromProduct = product =>     product.description
export const extractImagesFromProduct = product =>          product.images