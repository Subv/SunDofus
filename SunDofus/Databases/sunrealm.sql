/*
Navicat MySQL Data Transfer

Source Server         : Localhost
Source Server Version : 50612
Source Host           : localhost:3306
Source Database       : sunrealm

Target Server Type    : MYSQL
Target Server Version : 50612
File Encoding         : 65001

Date: 2013-08-18 14:40:19
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `accounts`
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `id` int(11) NOT NULL,
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `pseudo` varchar(255) NOT NULL,
  `gmLevel` int(11) NOT NULL,
  `communauty` int(11) NOT NULL,
  `subscription` datetime NOT NULL,
  `question` varchar(255) NOT NULL,
  `answer` varchar(255) NOT NULL,
  `connected` int(6) NOT NULL,
  `vote` int(11) NOT NULL,
  `points` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`username`,`pseudo`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of accounts
-- ----------------------------
INSERT INTO `accounts` VALUES ('1', 'test', 'test', '[Admin]Shaak', '1', '0', '2012-12-13 17:47:58', 'ADMIN ?', 'ADMIN', '0', '0', '0');
INSERT INTO `accounts` VALUES ('2', 'testt', 'tesst', 'test', '1', '0', '2013-01-17 18:04:18', '', '', '0', '0', '0');
INSERT INTO `accounts` VALUES ('3', 'shaak', 'ioski', 'Shaak', '0', '0', '2013-02-22 17:06:28', 'ADMIN ?', 'ADMIN ?', '0', '0', '0');
INSERT INTO `accounts` VALUES ('4', 'trolo', 'trolo', 'trooo', '0', '0', '2013-02-08 17:15:40', '', '', '0', '0', '0');

-- ----------------------------
-- Table structure for `accounts_characters`
-- ----------------------------
DROP TABLE IF EXISTS `accounts_characters`;
CREATE TABLE `accounts_characters` (
  `characterName` varchar(255) NOT NULL,
  `serverID` int(11) NOT NULL,
  `accountID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of accounts_characters
-- ----------------------------
INSERT INTO `accounts_characters` VALUES ('Xoyte', '6', '1');
INSERT INTO `accounts_characters` VALUES ('Weky', '6', '2');

-- ----------------------------
-- Table structure for `accounts_enemies`
-- ----------------------------
DROP TABLE IF EXISTS `accounts_enemies`;
CREATE TABLE `accounts_enemies` (
  `accID` int(11) NOT NULL,
  `targetPseudo` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of accounts_enemies
-- ----------------------------

-- ----------------------------
-- Table structure for `accounts_friends`
-- ----------------------------
DROP TABLE IF EXISTS `accounts_friends`;
CREATE TABLE `accounts_friends` (
  `accID` int(11) NOT NULL,
  `targetPseudo` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of accounts_friends
-- ----------------------------

-- ----------------------------
-- Table structure for `gameservers`
-- ----------------------------
DROP TABLE IF EXISTS `gameservers`;
CREATE TABLE `gameservers` (
  `Id` int(11) NOT NULL,
  `Ip` varchar(255) NOT NULL,
  `Port` int(11) NOT NULL,
  `PassKey` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of gameservers
-- ----------------------------
INSERT INTO `gameservers` VALUES ('6', '127.0.0.1', '5555', 'ioski');

-- ----------------------------
-- Table structure for `gifts`
-- ----------------------------
DROP TABLE IF EXISTS `gifts`;
CREATE TABLE `gifts` (
  `Id` int(11) NOT NULL,
  `Target` int(11) NOT NULL,
  `ItemID` int(11) NOT NULL,
  `Title` varchar(255) NOT NULL,
  `Message` text NOT NULL,
  `Image` text NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of gifts
-- ----------------------------
