# Xgbfi
XGBoost Feature Interactions &amp; Importance

### What is Xgbfi?
Xgbfi is a [XGBoost](https://github.com/dmlc/xgboost) model dump parser, which ranks features as well as feature interactions by different metrics. 

### The Metrics
 * **Gain**: Total gain of each feature or feature interaction
 * **FScore**: Amount of possible splits taken on a feature or feature interaction
 * **wFScore**: Amount of possible splits taken on a feature or feature interaction weighted by the probability of the splits to take place
 * **Average wFScore**: *wFScore* divided by *FScore*
 * **Average Gain**: *Gain* divided by *FScore*
 * **Expected Gain**: Total gain of each feature or feature interaction weighted by the probability to gather the gain

 
**Example:**

![](https://raw.githubusercontent.com/Far0n/xgbfi/master/doc/ScoresExample_small.png)

### Usage
