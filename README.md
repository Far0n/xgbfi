# Xgbfi
XGBoost Feature Interactions &amp; Importance

### What is Xgbfi?
Xgbfi is a [XGBoost](https://github.com/dmlc/xgboost) model dump parser, which ranks features as well as feature interactions by different metrics. 

### Siblings
[Xgbfir](https://github.com/limexp/xgbfir) - Python porting

### The Metrics
 * **Gain**: Total gain of each feature or feature interaction
 * **FScore**: Amount of possible splits taken on a feature or feature interaction
 * **wFScore**: Amount of possible splits taken on a feature or feature interaction weighted by the probability of the splits to take place
 * **Average wFScore**: *wFScore* divided by *FScore*
 * **Average Gain**: *Gain* divided by *FScore*
 * **Expected Gain**: Total gain of each feature or feature interaction weighted by the probability to gather the gain
 * **Average Tree Index**
 * **Average Tree Depth**

### Additional Features
 * **Leaf Statistics**
 * **Split Value Histograms**
 
**Example:**

![](https://raw.githubusercontent.com/Far0n/xgbfi/master/doc/ScoresExample_small.png)

### Usage
*[mono] XgbFeatureInteractions.exe [-help|options]*

### Quick Guide
a) Creating a feature map (fmap)
```python
def create_feature_map(fmap_filename, features):
"""
features: enumerable of feature names
"""
    outfile = open(fmap_filename, 'w')
    for i, feat in enumerate(features):
        outfile.write('{0}\t{1}\tq\n'.format(i, feat))
    outfile.close()
    
create_feature_map('xgb.fmap', features) 
```

b) Dumping a [XGBoost](https://github.com/dmlc/xgboost) model 
```python
gbdt.dump_model('xgb.dump',fmap='xgb.fmap', with_stats=True)
```

c) Editing Parameters in *XgbFeatureInteractions.exe.config*
```xml
<setting name="XgbModelFile" serializeAs="String">
    <value>xgb.dump</value>
</setting>
```

d) Running *[mono] XgbFeatureInteractions.exe* without cmd line parameters
