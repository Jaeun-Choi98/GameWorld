package rest

import (
	"log"
	"main/models"
	"net/http"
	"strconv"

	"github.com/gin-gonic/gin"
	"gorm.io/driver/mysql"
	"gorm.io/gorm"
)

// 이후에 DAO, Service 계층 구분 필요

type HandlerInterface interface {
	GetPlayerInfo(c *gin.Context)
	SignIn(c *gin.Context)
	SignUp(c *gin.Context)
}

type Handler struct {
	db *gorm.DB
}

func NewHandler() (HandlerInterface, error) {
	dsn := "root:0000@tcp(127.0.0.1:3306)/gameworld?charset=utf8mb4&parseTime=True&loc=Local"
	newDb, err := gorm.Open(mysql.Open(dsn), &gorm.Config{})
	if err != nil {
		return nil, err
	}
	return &Handler{db: newDb}, nil
}

func (h *Handler) GetPlayerInfo(c *gin.Context) {
	p := c.Param("id")
	id, _ := strconv.Atoi(p)

	var player []models.Player
	result := h.db.Where("user_id = ?", id).Find(&player)
	if result.Error != nil {
		log.Println(result.Error)
		c.JSON(http.StatusBadRequest, gin.H{"error": result.Error})
		return
	}
	c.JSON(http.StatusOK, player[0])
}

func (h *Handler) SignIn(c *gin.Context) {
	var reqUser models.User
	err := c.ShouldBindJSON(&reqUser)
	if err != nil {
		log.Println(err)
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	var dbUser models.User
	result := h.db.Where("email = ?", reqUser.Email).First(&dbUser)
	if result.Error != nil {
		log.Println(result.Error)
		c.JSON(http.StatusBadRequest, gin.H{"error": "존재하지 않는 이메일입니다."})
		return
	}

	if dbUser.Password != reqUser.Password {
		log.Println("비밀번호가 일치하지 않습니다.")
		c.JSON(http.StatusBadRequest, gin.H{"error": "비밀번호가 일치하지 않습니다."})
		return
	}

	c.JSON(http.StatusOK, dbUser)
}

func (h *Handler) SignUp(c *gin.Context) {
	var user models.User
	err := c.ShouldBindJSON(&user)
	if err != nil {
		log.Println(err)
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	result := h.db.Create(user)
	if result.Error != nil {
		log.Println(result.Error)
		c.JSON(http.StatusBadRequest, gin.H{"error": result.Error})
		return
	}

	c.JSON(http.StatusOK, user)
}
