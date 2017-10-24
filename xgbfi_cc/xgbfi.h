/*!
* Copyright 2017 by Contributors
* \file xgbfi.h
* \brief xgb feature interactions (xgbfi)
* \author Mathias Müller (Far0n)
*/
#ifndef XGBOOST_ANALYSIS_XGBFI_H_
#define XGBOOST_ANALYSIS_XGBFI_H_

//#include <xgboost/learner.h>
#include <string>
#include <vector>
#include <iostream>
#include <cstdio>
#include <fstream>
#include <sstream>
#include <memory>
#include <map>
#include <algorithm>
#include <cctype>
#include <unordered_set>
#include <set>


namespace xgbfi {
/*!
* \brief XGBoost Feature Interactions & Importance (Xgbfi)
* \param learner reference to instance of xgboost::Learner
* \param max_fi_depth upper bound for depth of interactions
* \param max_tree_depth upper bound for tree depth to be traversed
* \param max_deepening upper bound for tree deepening
* \param ntrees amount of trees to be traversed
* \param fmap path to fmap file, feature names "F1|F2|.." or empty string
* \return vector of strings formated like "F1|F2|..;stat1;stat2;.."
*/
//std::vector<std::string> GetFeatureInteractions(const xgboost::Learner& learner,
//                                                int max_fi_depth,
//                                                int max_tree_depth,
//                                                int max_deepening,
//                                                int ntrees,
//                                                const char* fmap);

  class XgbTreeNode;
  class XgbTree;
  class XgbModel;
  class FeatureInteraction;

  typedef std::vector<std::string> XgbModelDump;
  typedef std::shared_ptr<XgbTree> XgbTreePtr;
  typedef std::map<int, XgbTreeNode> XgbNodeList;
  typedef std::vector<XgbNodeList> XgbNodeLists;
  typedef std::map<std::string, FeatureInteraction> FeatureInteractions;
  typedef std::vector<XgbTreeNode> InteractionPath;
  typedef std::unordered_set<std::string> PathMemo;

  /*!
  * \brief xgbfi tree node type
  */
  enum XgbNodeType { None, BinaryFeature, NumericFeature, Leaf };

  /*!
  * \brief xgbfi xgb-model parser
  */
  class XgbModelParser {
  public:
    static XgbModel GetXgbModelFromDump(const std::string& file_path, int max_tress = -1);
    static XgbModel GetXgbModelFromDump(const XgbModelDump& dump, int max_tress = -1);

    static XgbModelDump ReadModelDump(const std::string& file_path, int max_trees = -1);
    static XgbTreeNode ParseXgbTreeNode(std::string* line);

  private:
    static void ConstructXgbTree(XgbTreePtr tree, const XgbNodeList &nodes);
    // static const std::regex node_regex_;
    // static const std::regex leaf_regex_;
  };

  /*!
  * \brief xgbfi xgb-model tree node
  */
  class XgbTreeNode {
  public:
    int number;
    std::string feature;
    double gain;
    double cover;
    int left_child;
    int right_child;
    double split_value;
    double leaf_value;
    bool is_leaf;

    XgbTreeNode() : number(0), feature(""), gain(0), cover(0),
      left_child(-1), right_child(-1), split_value(0), leaf_value(0), is_leaf(false) { }
  };


  /*!
  * \brief xgbfi xgb-model tree
  */
  class XgbTree {
  public:
    int number;
    XgbTreeNode root;
    XgbTreePtr left;
    XgbTreePtr right;

    explicit XgbTree(XgbTreeNode root, int number = 0)
      : number(number), root(root) { }
  };

  /*!
  * \brief xgbfi feature interaction
  */
  class FeatureInteraction {
  public:
    std::string name;
    int depth;
    double gain;
    double cover;
    double fscore;
    double w_fscore;
    double avg_w_fscore;
    double avg_gain;
    double expected_gain;

    FeatureInteraction() { }
    FeatureInteraction(const InteractionPath&  interaction,
      double gain, double cover, double path_proba, double depth, double fScore = 1);

    operator std::string() const {
      std::ostringstream oos;
      oos << name << ',' << depth << ',' << gain << ',' << fscore << ','
        << w_fscore << ',' << avg_w_fscore << ',' << avg_gain << ',' << expected_gain;
      return oos.str();
    }

    static std::string InteractionPathToStr(const InteractionPath& interaction_path,
      const bool encode_path = false, const bool sort_by_feature = true);

    static void Merge(FeatureInteractions* lfis, const FeatureInteractions& rfis);
  };

  /*!
  * \brief xgbfi xgb-model
  */
  class XgbModel {
  public:
    std::vector<XgbTreePtr> trees;
    int ntrees;

  private:
    int max_interaction_depth_;
    int max_tree_depth_;
    int max_deepening_;

    void CollectFeatureInteractions(XgbTreePtr tree, InteractionPath* cfi,
      double current_gain, double current_cover, double path_proba, int depth, int deepening,
      FeatureInteractions* tfis, PathMemo* memo);

  public:
    FeatureInteractions GetFeatureInteractions(int max_interaction_depth = -1,
      int max_tree_depth = -1,
      int max_deepening = -1);

    explicit XgbModel(int ntrees) : ntrees(ntrees) {
      trees.resize(ntrees);
    }
    XgbModel() : ntrees(0) { }
  };

  /*!
  * \brief xgbfi string utilities
  */
  class StringUtils {
  public:
    // static inline std::vector<std::string> split(const std::string& s, const std::string& regex) {
    //   std::regex re(regex);
    //   std::sregex_token_iterator itr {s.begin(), s.end(), re, -1 };
    //   return { itr, {} };
    // }

    static inline std::vector<std::string> split(const char* str, const char delim) {
      std::vector<std::string> tokens;
      std::istringstream iss(str);
      std::string tok;
      while (std::getline(iss, tok, delim)) {
        tokens.push_back(tok);
      }
      return tokens;
    }

    template <typename T>
    std::string join(const T& vec, const std::string& delim) {
      std::ostringstream s;
      for (const auto& i : vec) {
        if (&i != &vec[0]) {
          s << delim;
        }
        s << i;
      }
      return s.str();
    }

    static inline void ltrim(std::string* s) {
      s->erase(s->begin(), std::find_if(s->begin(), s->end(), [](int ch) {
        return !std::isspace(ch);
      }));
    }

    static inline void rtrim(std::string* s) {
      s->erase(std::find_if(s->rbegin(), s->rend(), [](int ch) {
        return !std::isspace(ch);
      }).base(), s->end());
    }

    static inline void trim(std::string* s) {
      ltrim(s);
      rtrim(s);
    }
  };
}  // namespace xgbfi
#endif  // XGBOOST_ANALYSIS_XGBFI_H_
