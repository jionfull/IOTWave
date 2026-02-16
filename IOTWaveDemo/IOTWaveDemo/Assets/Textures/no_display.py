from PIL import Image, ImageDraw

"""
无表示纹理生成脚本
独立运行生成无表示纹理
"""

def create_no_display_texture():
    """创建无表示纹理"""
    # 创建16x16的黑色背景图像
    img = Image.new("RGB", (16, 16), color="#000000")
    draw = ImageDraw.Draw(img)
    
    # 灰色填充整个图像
    draw.rectangle([0, 0, 16, 16], fill="#808080")
    
    return img

def main():
    """主函数"""
    print("生成无表示纹理...")
    img = create_no_display_texture()
    img.save("no_display.png")
    print("无表示纹理已生成: no_display.png")

if __name__ == "__main__":
    main()