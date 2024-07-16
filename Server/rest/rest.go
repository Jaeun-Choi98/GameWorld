package rest

import (
	"github.com/gin-gonic/gin"
)

func RunServer(address string) error {
	h, err := NewHandler()
	if err != nil {
		return err
	}
	return RunWithHandler(address, h)
}

func RunWithHandler(address string, h HandlerInterface) error {
	r := gin.Default()
	r.GET("/players/:id", h.GetPlayerInfo)
	r.POST("/players/signin", h.SignIn)
	r.POST("/players/signup", h.SignUp)
	r.POST("/players/save-playerinfo", h.SavePlayerInfo)
	return r.Run(address)
}
