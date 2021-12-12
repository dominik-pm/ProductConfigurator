import { createSelector } from 'reselect'


// input selectors:
const selectOptionId = (state, optionId) =>             optionId

const selectAllOptionIncompatibilities = state =>       state.configuration.configuration.rules.incompatibilites
const selectAllOptionRequirements = state =>            state.configuration.configuration.rules.requirements
const selectReplacementGroups = state =>                state.configuration.configuration.rules.replacementGroups

export const selectBasePrice = state =>                 state.configuration.configuration.rules.basePrice
export const selectPriceList = state =>                 state.configuration.configuration.rules.priceList
export const selectConfigurationName = state =>         state.configuration.configuration.name
export const selectConfigurationDescription = state =>  state.configuration.configuration.description

export const selectOptions = state =>                   state.configuration.configuration.options
export const selectOptionGroups = state =>              state.configuration.configuration.optionGroups
export const selectOptionSections = state =>            state.configuration.configuration.optionSections
export const selectSelectedOptions = state =>           state.configuration.selectedOptions

export const getOption = createSelector([selectOptions, selectOptionId], (options, id) => {
    // console.log('Option output')
    // console.log('---> ' + id)
    return options.find(o => o.id === id)
})

export const getIsOptionSelected = createSelector([selectSelectedOptions, selectOptionId], (selectedOptions, id) => {
    // console.log('Is option selected output')
    return selectedOptions.includes(id)
})

// all options that are in the same replacement group (or null if the option is in no replacement group)
export const getOptionReplacementGroup = createSelector([selectReplacementGroups, selectOptionId], (replacementGroups, id) => {
    // console.log('Option replacement groups output')

    for (let groupName of Object.keys(replacementGroups)) {
        const replacementGroup = replacementGroups[groupName]
        if (replacementGroup.includes(id)) {
            return replacementGroup
        }
    }

    return null
})

// -- getIsOptionSelectable -->
const getOptionIncompatibilities = createSelector([selectAllOptionIncompatibilities, selectOptionId], (incompatibilites, optionId) => {
    // console.log('Option Incompatitbilities output')
    return incompatibilites[optionId]
})
const getIsOptionCompatible = createSelector([selectSelectedOptions, getOptionIncompatibilities], (selectedOptions, incompatibilites) => {
    // returns an array:
    //  -> 1. index: if the option is compatible or not
    //  -> 2. index: options that are incompatible

    if (!incompatibilites) return [true, null]

    let compatible = true
    let incompatibleOptions = []
    incompatibilites.forEach(incompatibility => {
        // if there is an incompatibility in the selected options, the option is not compatible
        if (selectedOptions.includes(incompatibility)) {
            compatible = false
            incompatibleOptions.push(incompatibility)
        }
    })
    return [compatible, incompatibleOptions]
})
const getOptionRequirements = createSelector([selectAllOptionRequirements, selectOptionId], (requirements, optionId) => {
    // console.log('Option Requirements output')
    return requirements[optionId]
})
const getIsOptionRequirementsMet = createSelector([selectSelectedOptions, getOptionRequirements], (selectedOptions, requirements) => {
    // returns an array:
    //  -> 1. index: if the options requirements are met or not (required options are selected)
    //  -> 2. index: options that are required for this option

    // console.log('Is option requirements met output')

    if (!requirements) return [true, null] // if there are no requirements, return true
    
    let reqMet = true
    let requiredOptions = []
    requirements.forEach(requirement => {
        // if there is a requirement not in the selected options, the option does not meet its requirements
        if (!selectedOptions.includes(requirement)) {
            reqMet = false
            requiredOptions.push(requirement)
        }
    })
    return [reqMet, requiredOptions]
})
export const getIsOptionSelectable = createSelector([getIsOptionRequirementsMet, getIsOptionCompatible], (isRequirementsMet, isCompatible) => {
    // console.log('Is option selectable output')

    if (!isRequirementsMet[0])     return [false, isRequirementsMet[1].join(', ') + (isRequirementsMet[1].length > 1 ? ' are ' : ' is ') + 'required!']
    if (!isCompatible[0])          return [false, 'Not compatible with: ' + isCompatible[1].join(', ')]

    return [true, '']
})
// <-- getIsOptionSelectable --

export const getOptionPrice = createSelector([selectPriceList, selectOptionId], (priceList, id) => {
    // console.log('Option price output')
    return priceList[id]
})

// the total price of all selected options plus the base price
export const getCurrentPrice = createSelector([selectSelectedOptions, selectBasePrice, selectPriceList], (selectedOptions, basePrice, priceList) => {
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

// the options that cant be used if this option is deselected
export const getDependentOptionsDeselect = createSelector(
    [selectSelectedOptions, selectOptionId, selectAllOptionRequirements], 
    (selectedOptions, selectedOptionId, requirements) => {

    let dependencies = dependenciesFromDependencyLists(selectedOptionId, requirements)

    // only get the dependencies from the selected options
    dependencies = dependencies.filter(d => selectedOptions.includes(d))

    return dependencies
})
// the options that cant be used if this option selected
export const getDependentOptionsSelect = createSelector(
    [selectSelectedOptions, selectOptionId, selectAllOptionIncompatibilities], 
    (selectedOptions, selectedOptionId, incompatibilites) => {

    let dependencies = dependenciesFromDependencyLists(selectedOptionId, incompatibilites)
    
    // only get the dependencies from the selected options
    dependencies = dependencies.filter(d => selectedOptions.includes(d))

    return dependencies
})
function dependenciesFromDependencyLists(dependentOption, ...dependencyLists) {
    let dependencies = []

    // for every dependency list
    for (const dependencyList of dependencyLists) {

        // for every dependency in the dependency list
        for (const dependency in dependencyList) {
            // check if the dependent option is included in the dependencies
            if (dependencyList[dependency].includes(dependentOption)) {
                // add the dependency (the option that depends on the dependentOption)
                dependencies.push(dependency)
            }
        }

    }

    return dependencies
}