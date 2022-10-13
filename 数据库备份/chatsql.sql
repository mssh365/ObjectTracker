/*
 Navicat Premium Data Transfer

 Source Server         : data
 Source Server Type    : MySQL
 Source Server Version : 50738
 Source Host           : localhost:3306
 Source Schema         : chatsql

 Target Server Type    : MySQL
 Target Server Version : 50738
 File Encoding         : 65001

 Date: 03/07/2022 19:59:32
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for fan
-- ----------------------------
DROP TABLE IF EXISTS `fan`;
CREATE TABLE `fan`  (
  `name` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `follow` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  PRIMARY KEY (`name`, `follow`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of fan
-- ----------------------------
INSERT INTO `fan` VALUES ('2', '2');
INSERT INTO `fan` VALUES ('2', '3');
INSERT INTO `fan` VALUES ('3', '2');
INSERT INTO `fan` VALUES ('3', '3');
INSERT INTO `fan` VALUES ('4', '2');

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `password` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `admin` int(255) NOT NULL,
  `status` int(255) NOT NULL,
  PRIMARY KEY (`name`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES ('1', '1', 1, 0);
INSERT INTO `user` VALUES ('2', '2', 0, 0);
INSERT INTO `user` VALUES ('3', '3', 0, 0);
INSERT INTO `user` VALUES ('4', '4', 0, 0);

SET FOREIGN_KEY_CHECKS = 1;
