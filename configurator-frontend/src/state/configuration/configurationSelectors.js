import { createSelector } from 'reselect'


// input selectors:
const selectOptionId = (state, optionId) => optionId

const selectConfiguration = state =>        state.configuration.configuration
const selectBasePrice = state =>            state.configuration.configuration.rules.basePrice

const selectOptions = state =>              state.configuration.configuration.options
const selectOptionGroups = state =>         state.configuration.configuration.optionGroups
const selectOptionSections = state =>       state.configuration.configuration.optionSections
const selectRules = state =>                state.configuration.configuration.rules
const selectPriceList = state =>            state.configuration.configuration.rules.priceList
const selectSelectedOptions = state =>      state.configuration.selectedOptions


// output selectors:
export const getConfigurationName = createSelector([selectConfiguration], (config) => {
    console.log('Configuration name output')
    return config.name
})
export const getConfigurationDescription = createSelector([selectConfiguration], (config) => {
    console.log('Configuration description output')
    return config.description
})

export const getOptions = createSelector([selectOptions], (options) => {
    console.log('Options output')
    return options
})
export const getSelectedOptions = createSelector([selectSelectedOptions], (optionIds) => {
    console.log('Selected Options output')
    return optionIds
})
export const getOptionGroups = createSelector([selectOptionGroups], (groups) => {
    console.log('Option Groups output')
    return groups
})
export const getOptionSections = createSelector([selectOptionSections], (sections) => {
    console.log('Option Sections output')
    return sections
})
export const getRules = createSelector([selectRules], (rules) => {
    console.log('Rules output')
    return rules
})
export const getPriceList = createSelector([selectPriceList], (prices) => {
    console.log('PriceList output')
    return prices
})


export const getOption = createSelector([selectOptions, selectOptionId], (options, id) => {
    console.log('Option output')
    console.log('---> ' + id)
    return options.find(o => o.id === id)
})
export const getIsOptionSelected = createSelector([getSelectedOptions, selectOptionId], (selectedOptions, id) => {
    console.log('Is option selected output')
    return selectedOptions.includes(id)
})
export const getOptionPrice = createSelector([selectPriceList, selectOptionId], (priceList, id) => {
    console.log('Option price output')
    return priceList[id]
})
export const getBasePrice = createSelector([selectBasePrice], (basePrice) => {
    console.log('Base price output')
    return basePrice
})
export const getCurrentPrice = createSelector([getSelectedOptions, selectBasePrice, selectPriceList], (selectedOptions, basePrice, priceList) => {
    console.log('Current Price output')
    const price = selectedOptions.reduce((total, optionId) => {
        if (priceList[optionId]) {
            return total + priceList[optionId]
        } else {
            return total
        }
    }, basePrice)
    return price
})